import { UrlInfoResponse } from "@src/models/Url";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

const UrlInfoPage = () => {
  const { urlId } = useParams<{ urlId: string }>();

  const [urlInfo, setUrlInfo] = useState<UrlInfoResponse>();

  useEffect(() => {
    if (!urlId) return;

    const abortController = new AbortController();

    const fetchUrlInfo = async () => {
      try {
        const data = await UrlsShortenerService.getInfo(urlId, { signal: abortController.signal });
        console.log(data);
        setUrlInfo(data);
      } catch (e: any) {
        console.error("Unable to get url info:", e.message);
      }
    };

    fetchUrlInfo();

    return () => abortController.abort();
  }, [urlId]);

  return (
    <div className="bg-slate-300 w-full">
      <div className="flex items-center gap-3 text-2xl flex-col">
        <p className="text-4xl break-all">Info about url: {urlInfo?.urlOriginal}</p>
        <p className="break-all">Shortened version: {urlInfo?.urlShortened}</p>
        <p className="break-all">Url id: {urlInfo?.id}</p>
        <p className="break-all">Creation time: {new Date(urlInfo?.createdAt!).toLocaleString()}</p>
        <p className="break-all">Creator id: {urlInfo?.user.id}</p>
        <p className="break-all">Created by: {urlInfo?.user.username}</p>
      </div>
    </div>
  );
};

export default UrlInfoPage;
