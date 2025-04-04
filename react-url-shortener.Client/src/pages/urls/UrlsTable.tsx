import Garbage from "@src/components/svgr/Garbage";
import { Roles } from "@src/services/api/models/Auth";
import UrlsTableHeader from "./UrlsTableHeader";
import UrlsTableRow from "./UrlsTableRow";
import { useUrls } from "./UrlsContext";
import { useAuth } from "@src/hooks/useAuth";
import { UrlResponse } from "@src/services/api/models/Url";
import { handleError } from "@src/utils/helpers";
import Button from "@src/components/ui/Buttons/Button";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";
import CustomLink from "@src/components/ui/Links/CustomLink";
import SwgButton from "@src/components/ui/Buttons/SwgButton";
import { toast } from "@src/hooks/useToast";

type UrlsTableProps = {
  onShortUrlClick: (url: UrlResponse) => void;
};

const UrlsTable = ({ onShortUrlClick }: UrlsTableProps) => {
  const { user, hasRole } = useAuth();
  const { urls, fetchUrls, totalCount, totalPages, pageNumber, pageSize } = useUrls();

  const handleDeleteUrl = async (urlId: string) => {
    try {
      await UrlsShortenerService.delete(urlId);
      toast.success("Url deleted successfully");
    } catch (e: any) {
      handleError(e, toast.danger);
    }
  };

  const handleBackClick = async () => {
    await fetchUrls(pageNumber - 1, pageSize);
  };

  const handleForwardClick = async () => {
    await fetchUrls(pageNumber + 1, pageSize);
  };

  return (
    <div className="flex flex-col gap-2">
      <table className="shadow overflow-hidden w-full rounded-2xl bg-gradient-to-r dark:from-gray-700 dark:to-gray-900 from-gray-200 to-gray-200">
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
                    <CustomLink
                      to={url.urlOriginal}
                      className="px-2 py-1 text-blue-400 hover:text-blue-500 rounded"
                    >
                      {content}
                    </CustomLink>
                  );
                }

                if (index === 2) {
                  const link = (
                    <button
                      onClick={() => onShortUrlClick(url)}
                      className="border-b-2 border-transparent cursor-pointer link-primary link-default"
                    >
                      {content}
                    </button>
                  );

                  return (
                    <div className="flex justify-between gap-2">
                      {link}
                      {url.userId === user?.id || hasRole(Roles.Admin) ? (
                        <SwgButton onClick={() => handleDeleteUrl(url.id)}>
                          <Garbage className="fill-gray-800 dark:fill-gray-200" role="button" aria-label="Delete url" cursor="pointer" />
                        </SwgButton>
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
          &lt;
        </Button>
        <p>total count: {totalCount}</p>
        <p>total pages: {totalPages}</p>
        <p>page number: {pageNumber}</p>
        <p>page size: {pageSize}</p>
        <Button disabled={pageNumber >= totalPages} onClick={handleForwardClick}>
          &gt;
        </Button>
      </div>
    </div>
  );
};

export default UrlsTable;
