import Garbage from "@src/components/svgr/Garbage";
import { Roles } from "@src/services/api/models/Auth";
import { Link } from "react-router-dom";
import UrlsTableHeader from "./UrlsTableHeader";
import UrlsTableRow from "./UrlsTableRow";
import { useUrls } from "./UrlsContext";
import { useAuth } from "@src/hooks/useAuth";
import { UrlResponse } from "@src/services/api/models/Url";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";
import { handleError } from "@src/utils/helpers";
import { useToast } from "@src/hooks/useToast";

type UrlsTableProps = {
  onShortUrlClick: (url: UrlResponse) => void;
};

const UrlsTable = ({ onShortUrlClick }: UrlsTableProps) => {
  const { user, hasRole } = useAuth();
  const { urls, deleteUrl } = useUrls();
  const { success, danger } = useToast();

  const handleDeleteUrl = async (urlId: string) => {
    try {
      await UrlsShortenerService.delete(urlId);
      deleteUrl(urlId);
      success("Url deleted successfully");
    } catch (e: any) {
      handleError(e, danger);
    }
  };

  return (
    <table className="w-full border-separate">
      <UrlsTableHeader columns={["Id", "Original URL", "Short URL"]} />
      <tbody className="divide-y-1">
        {urls.map((url) => (
          <UrlsTableRow
            key={url.id}
            url={url}
            columns={["id", "urlOriginal", "urlShortened"]}
            isHighlighted={user?.id === url.userId}
            wrapper={(index, content) => {
              if (index === 0) {
                return <div className="whitespace-nowrap">{content}</div>;
              }

              if (index === 1) {
                return (
                  <Link
                    to={url.urlOriginal}
                    className="px-2 py-1 text-blue-400 hover:text-blue-500 rounded"
                  >
                    {content}
                  </Link>
                );
              }

              if (index === 2) {
                const link = (
                  <button
                    onClick={() => onShortUrlClick(url)}
                    className="px-2 py-1 whitespace-nowrap cursor-pointer text-blue-400 hover:text-blue-500 rounded"
                  >
                    {content}
                  </button>
                );

                return (
                  <div className="flex justify-between">
                    {link}
                    {url.userId === user?.id || hasRole(Roles.Admin) ? (
                      <Garbage onClick={() => handleDeleteUrl(url.id)} cursor="pointer" />
                    ) : (
                      <></>
                    )}
                  </div>
                );
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
