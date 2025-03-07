import Garbage from "@src/components/svgr/Garbage";
import { Roles } from "@src/services/api/models/Auth";
import { Link } from "react-router-dom";
import UrlsTableHeader from "./UrlsTableHeader";
import UrlsTableRow from "./UrlsTableRow";
import { useUrls } from "./UrlsContext";
import { useAuth } from "@src/hooks/useAuth";
import { UrlResponse } from "@src/services/api/models/Url";
import { handleError } from "@src/utils/helpers";
import { useToast } from "@src/hooks/useToast";
import Button from "@src/components/ui/Button";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";

type UrlsTableProps = {
  onShortUrlClick: (url: UrlResponse) => void;
};

const UrlsTable = ({ onShortUrlClick }: UrlsTableProps) => {
  const { user, hasRole } = useAuth();
  const { urls, fetchUrls, totalCount, totalPages, pageNumber, pageSize } = useUrls();
  const { success, danger } = useToast();

  const handleDeleteUrl = async (urlId: string) => {
    try {
      await UrlsShortenerService.delete(urlId);
      success("Url deleted successfully");
    } catch (e: any) {
      handleError(e, danger);
    }
  };

  const handleBackClick = async () => {
    await fetchUrls(pageNumber - 1, pageSize);
  };

  const handleForwardClick = async () => {
    await fetchUrls(pageNumber + 1, pageSize);
  };

  return (
    <>
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
      <div className="flex items-center gap-2 justify-center">
        <Button disabled={pageNumber <= 1} onClick={handleBackClick}>
          Back
        </Button>
        <p>total count: {totalCount}</p>
        <p>total pages: {totalPages}</p>
        <p>page number: {pageNumber}</p>
        <p>page size: {pageSize}</p>
        <Button disabled={pageNumber >= totalPages} onClick={handleForwardClick}>
          Forward
        </Button>
      </div>
    </>
  );
};

export default UrlsTable;
