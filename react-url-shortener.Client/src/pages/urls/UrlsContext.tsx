import { UrlResponse } from "@src/services/api/models/Url";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";
import React, { useContext, useEffect, useMemo, useState } from "react";

type UrlsContextType = {
  urls: UrlResponse[];
  totalCount: number;
  totalPages: number;
  pageNumber: number;
  pageSize: number;
  fetchUrls: (pageNumber: number, pageSize: number, signal?: AbortSignal) => Promise<void>;
  deleteUrl: (urlId: string) => Promise<void>;
  createUrl: (originalUrl: string) => Promise<void>;
};

const UrlsContext = React.createContext({} as UrlsContextType);

type UrlsContextProviderProps = { children: React.ReactNode };

export const UrlsContextProvider = ({ children }: UrlsContextProviderProps) => {
  const [urls, setUrls] = useState<UrlResponse[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(0);

  useEffect(() => {
    const abortController = new AbortController();

    fetchUrls(1, 5, abortController.signal);

    return () => abortController.abort();
  }, []);

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
      console.error("Unable to fetch urls:", e.message);
    }
  };

  const createUrl = async (originalUrl: string) => {
    const urlResponse = await UrlsShortenerService.create(originalUrl);
    if (!urlResponse) return;

    const newUrls = [...urls, urlResponse];
    if (newUrls.length > pageSize) {
      await fetchUrls(pageNumber + 1, 5);
    } else {
      setUrls(newUrls);
    }
  };

  const deleteUrl = async (urlId: string) => {
    await UrlsShortenerService.delete(urlId);
    const newUrls = urls.filter((u) => u.id !== urlId);
    if (newUrls.length === 0) {
      await fetchUrls(pageNumber - 1, 5);
    } else {
      setUrls(newUrls);
    }
  };

  const value = useMemo(
    () => ({
      urls,
      fetchUrls,
      deleteUrl,
      createUrl,
      totalCount,
      totalPages,
      pageNumber,
      pageSize,
    }),
    [urls]
  );

  return <UrlsContext.Provider value={value}>{children}</UrlsContext.Provider>;
};

export const useUrls = () => {
  const urlsContext = useContext(UrlsContext);

  if (!urlsContext) {
    throw new Error("urlsContext must be used within a UrlsContextProvider");
  }

  return urlsContext;
};
