import AuthComponent from "@src/components/HOC/AuthComponent";
import AddUrlSection from "./AddUrlSection";
import UrlsTable from "./UrlsTable";
import { UrlResponse } from "@src/models/Url";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";
import { useState, useEffect } from "react";

const UrlsMain = () => {
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

  const hanldeUrlCreated = (data: any) => {
    setUrls([...urls, data]);
  };

  const handleUrlDeleted = (urlId: string) => {
    setUrls((prev) => prev.filter((u) => u.id !== urlId));
  };

  return (
    <>
      <AuthComponent>
        <AddUrlSection className="max-w-2xl" onUrlCreated={hanldeUrlCreated} />
      </AuthComponent>
      {urls.length ? (
        <UrlsTable onUrlDeleted={handleUrlDeleted} urls={urls} />
      ) : (
        "Urls table is empty :("
      )}
    </>
  );
};

export default UrlsMain;
