import ErrorScreen, { ErrorScreenProps } from "@src/components/ui/ErrorScreen";
import LoadingScreen from "@src/components/ui/LoadingScreen";
import { UrlResponse } from "@src/services/api/models/Url";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";
import React, { useContext, useEffect, useMemo, useState } from "react";
import useUrlsHubConnection from "./hooks/useUrlsHubConnection";
import { useAuth } from "@src/hooks/useAuth";

type UrlsContextType = {
  urls: UrlResponse[];
  totalCount: number;
  totalPages: number;
  pageNumber: number;
  pageSize: number;
  fetchUrls: (pageNumber: number, pageSize: number, signal?: AbortSignal) => Promise<void>;
};

const UrlsContext = React.createContext({} as UrlsContextType);

type UrlsContextProviderProps = { children: React.ReactNode };

export const UrlsContextProvider = ({ children }: UrlsContextProviderProps) => {
  const { user } = useAuth();

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<ErrorScreenProps | null>(null);

  const connection = useUrlsHubConnection();

  const [urls, setUrls] = useState<UrlResponse[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(0);

  const fetchUrls = async (pageNumber: number = 1, pageSize: number = 5, signal?: AbortSignal) => {
    try {
      const data = await UrlsShortenerService.getAll(pageNumber, pageSize, { signal: signal });
      if (data) {
        setUrls(data.items);
        setTotalCount(data.totalCount);
        setTotalPages(data.totalPages);
        setPageNumber(data.pageNumber);
        setPageSize(data.pageSize);
      }
    } catch (e: any) {
      setError({ status: e.status, message: e.response.data.errors[0] });
      console.error("Unable to fetch urls:", e.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const abortController = new AbortController();

    fetchUrls(1, 5, abortController.signal);

    return () => abortController.abort();
  }, []);

  useEffect(() => {
    if (!connection) return;

    const registerSignalREventHandlers = () => {
      connection.off("CreateUrl");
      connection.on("CreateUrl", async (urlResponse: UrlResponse) => {
        setTotalCount((prev) => prev + 1);

        if (totalPages === 0) {
          setTotalPages((prev) => prev + 1);
        }

        const isNewUrlsLenghtBiggerThanPageSize = urls.length + 1 > pageSize;
        const isLastPage = pageNumber === totalPages;
        const isCurrentUserCreator = urlResponse.userId === user?.id;

        if (isCurrentUserCreator) {
          if (isNewUrlsLenghtBiggerThanPageSize) {
            await fetchUrls(totalPages + 1, 5);
          } else {
            await fetchUrls(totalPages, 5);
          }
        }

        if (!isNewUrlsLenghtBiggerThanPageSize && isLastPage) {
          const newUrls = [...urls, urlResponse];
          setUrls(newUrls);
        }
      });

      connection.off("DeleteUrl");
      connection.on("DeleteUrl", async (_urlId: string) => {
        setTotalCount((prev) => prev - 1);

        const isNewUrlsLengthIsZero = urls.length - 1 === 0;
        const isFirstPage = pageNumber === 1;

        if (isNewUrlsLengthIsZero) {
          if (isFirstPage) {
            await fetchUrls(pageNumber, 5);
          } else {
            await fetchUrls(pageNumber - 1, 5);
          }
        } else {
          await fetchUrls(pageNumber, 5);
        }
      });
    };

    registerSignalREventHandlers();
  }, [connection, urls, pageNumber, pageSize, totalPages, totalCount]);

  const value = useMemo(
    () => ({
      urls,
      fetchUrls,
      totalCount,
      totalPages,
      pageNumber,
      pageSize,
    }),
    [urls, totalCount, totalPages, pageNumber, pageSize]
  );

  if (loading) return <LoadingScreen />;
  if (error) return <ErrorScreen {...error} />;

  return <UrlsContext.Provider value={value}>{children}</UrlsContext.Provider>;
};

export const useUrls = () => {
  const urlsContext = useContext(UrlsContext);

  if (!urlsContext) {
    throw new Error("urlsContext must be used within a UrlsContextProvider");
  }

  return urlsContext;
};
