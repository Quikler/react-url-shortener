import UrlsTableHeader from "@src/pages/urls/UrlsTableHeader";
import UrlsTableRow from "@src/pages/urls/UrlsTableRow";
import { useAuth } from "@src/hooks/useAuth";
import { UrlResponse } from "@src/models/Url";
import { Link } from "react-router-dom";
import Garbage from "@src/components/svgr/Garbage";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";

type UrlsTableProps = {
  urls: UrlResponse[];
  onUrlDeleted: (urlId: string) => void;
};

const UrlsTable = ({ urls, onUrlDeleted }: UrlsTableProps) => {
  const { user } = useAuth();

  const handleDeleteUrl = async (urlId: string) => {
    try {
      await UrlsShortenerService.delete(urlId);
      onUrlDeleted(urlId);
    } catch (e: any) {
      console.error("Unable to delete url:", e.message);
    }
  };

  return (
    <table className="rounded-xl">
      <UrlsTableHeader columns={["Id", "Original URL", "Short URL"]} />
      <tbody className="divide-y-1 divide-black-300">
        {urls.map((url) => (
          <UrlsTableRow
            key={url.id}
            url={url}
            columns={["id", "urlOriginal", "urlShortened"]}
            isHighlighted={user?.id === url.userId}
            wrapper={(index, content) => {
              if (index === 2) {
                const link = (
                  <Link
                    to={url.urlShortened}
                    target="_blank"
                    className="px-2 py-1 bg-blue-500 text-white rounded"
                  >
                    {content}
                  </Link>
                );
                if (url.userId === user?.id) {
                  return (
                    <div className="flex gap-2">
                      {link}
                      <Garbage onClick={() => handleDeleteUrl(url.id)} cursor="pointer" />
                    </div>
                  );
                }
              }
              return content;
            }}
          />
        ))}
      </tbody>
    </table>
  );
};

export default UrlsTable;
