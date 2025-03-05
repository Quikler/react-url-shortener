import { UrlResponse } from "@src/services/api/models/Url";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";
import React, { useContext, useEffect, useMemo, useState } from "react";

type UrlsContextType = {
  urls: UrlResponse[];
  deleteUrl: (urlId: string) => void;
  createUrl: (url: UrlResponse) => void;
};

const UrlsContext = React.createContext({} as UrlsContextType);

type UrlsContextProviderProps = { children: React.ReactNode };

export const UrlsContextProvider = ({ children }: UrlsContextProviderProps) => {
  const [urls, setUrls] = useState<UrlResponse[]>([]);

  useEffect(() => {
    const abortController = new AbortController();

    const fetchUrls = async () => {
      try {
        const data = await UrlsShortenerService.getAll({ signal: abortController.signal });
        if (data) {
          setUrls(data.items);
        }
      } catch (e: any) {
        console.error("Unable to fetch urls:", e.message);
      }
    };

    fetchUrls();

    return () => abortController.abort();
  }, []);

  const createUrl = (url: UrlResponse) => {
    setUrls([...urls, url]);
  };

  const deleteUrl = (urlId: string) => {
    setUrls((prev) => prev.filter((u) => u.id !== urlId));
  };

  const value = useMemo(
    () => ({
      urls,
      deleteUrl,
      createUrl,
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
