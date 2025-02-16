import { useEffect, useState } from "react";
import AddUrlSection from "./AddUrlSection";
import UrlsTable from "./UrlsTable";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";
import { UrlResponse } from "@src/models/Url";
import { useAuth } from "@src/hooks/useAuth";

const UrlsPage = () => {
  const { isUserLoggedIn } = useAuth();

  const [urls, setUrls] = useState<UrlResponse[]>([]);

  useEffect(() => {
    const abortController = new AbortController();

    const fetchUrls = async () => {
      try {
        const data = await UrlsShortenerService.getAll({ signal: abortController.signal });
        if (data) {
          setUrls(data);
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
    <div className="flex flex-col">
      <div className=" w-full mx-auto inline-block align-middle">
        <div className="flex flex-col gap-6 items-center p-6 my-24 border rounded-lg border-gray-300">
          {isUserLoggedIn() && (
            <AddUrlSection className="max-w-2xl" onUrlCreated={hanldeUrlCreated} />
          )}
          {urls.length ? (
            <UrlsTable onUrlDeleted={handleUrlDeleted} urls={urls} />
          ) : (
            "Urls table is empty :("
          )}
        </div>
      </div>
    </div>
  );
};

export default UrlsPage;
