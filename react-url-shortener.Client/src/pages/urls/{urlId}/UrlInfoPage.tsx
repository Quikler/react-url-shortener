import ErrorScreen, { ErrorScreenProps } from "@src/components/ui/ErrorScreen";
import LoadingScreen from "@src/components/ui/LoadingScreen";
import { UrlInfoResponse } from "@src/services/api/models/Url";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

const UrlInfoPage = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<ErrorScreenProps | null>(null);

  const { urlId } = useParams<{ urlId: string }>();

  const [urlInfo, setUrlInfo] = useState<UrlInfoResponse>();

  useEffect(() => {
    if (!urlId) return;

    const abortController = new AbortController();

    const fetchUrlInfo = async () => {
      try {
        const data = await UrlsShortenerService.getInfo(urlId, { signal: abortController.signal });
        setUrlInfo(data);
      } catch (e: any) {
        console.error("Unable to get url info:", e.message);
        console.log(e)
        setError({ status: e.status, message: e.response.data.errors[0] });
      } finally {
        setLoading(false);
      }
    };

    fetchUrlInfo();

    return () => abortController.abort();
  }, [urlId]);

  if (loading) return <LoadingScreen />
  if (error) return <ErrorScreen {...error} />

  return (
    <div className="p-8 bg-white shadow rounded-2xl border-black flex flex-col gap-3 items-center">
      <p className="text-4xl">Info about url</p>
      <p className="text-2xl break-words">Original url: {urlInfo?.urlOriginal}</p>
      <p className="text-2xl break-words">Shortened version: {urlInfo?.urlShortened}</p>
      <p className="break-words">Url id: {urlInfo?.id}</p>
      <p className="break-words">Creation time: {new Date(urlInfo?.createdAt!).toLocaleString()}</p>
      <p className="break-words">Creator id: {urlInfo?.user.id}</p>
      <p className="break-words text-xl">Created by: {urlInfo?.user.username}</p>
    </div>
  );
};

export default UrlInfoPage;
