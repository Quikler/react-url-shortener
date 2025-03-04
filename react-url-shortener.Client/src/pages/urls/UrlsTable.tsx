import UrlsTableHeader from "@src/pages/urls/UrlsTableHeader";
import UrlsTableRow from "@src/pages/urls/UrlsTableRow";
import { useAuth } from "@src/hooks/useAuth";
import { UrlResponse } from "@src/models/Url";
import Garbage from "@src/components/svgr/Garbage";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";
import { Roles } from "@src/models/Auth";
import Modal from "@src/components/ui/Modal";
import { useState } from "react";
import Button from "@src/components/ui/Button";
import ButtonLink from "@src/components/ui/ButtonLink";
import { Link } from "react-router-dom";
import { useToast } from "@src/hooks/useToast";

type UrlsTableProps = {
  urls: UrlResponse[];
  onUrlDeleted: (urlId: string) => void;
};

const UrlsTable = ({ urls, onUrlDeleted }: UrlsTableProps) => {
  const { user, hasRole, isUserLoggedIn } = useAuth();
  const { success } = useToast();

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedUrl, setSelectedUrl] = useState<UrlResponse | null>(null);

  const handleDeleteUrl = async (urlId: string) => {
    try {
      await UrlsShortenerService.delete(urlId);
      onUrlDeleted(urlId);
      success("Url deleted successfully");
    } catch (e: any) {
      console.error("Unable to delete url:", e.message);
    }
  };

  const handleShortUrlClick = (url: UrlResponse) => {
    setIsModalOpen(true);
    setSelectedUrl(url);
  };

  return (
    <>
      <Modal title="Choose an option" isOpen={isModalOpen} onClose={() => setIsModalOpen(false)}>
        <div>
          <div>
            <span className="text-xl">Url:</span> {selectedUrl?.urlShortened}
          </div>
          <div className="mt-4 flex gap-2">
            <ButtonLink to={(selectedUrl && selectedUrl?.urlShortened) || "#"} target="_blank">
              Redirect
            </ButtonLink>
            {isUserLoggedIn() && (
              <ButtonLink to={`/urls/${selectedUrl?.id}/info`} variant="info">
                Info
              </ButtonLink>
            )}
            <Button
              onClick={() => selectedUrl && navigator.clipboard.writeText(selectedUrl.urlShortened)}
              variant="secondary"
            >
              Copy
            </Button>
          </div>
        </div>
      </Modal>
      <div className="bg-white shadow rounded-2xl border-black">
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
                        onClick={() => handleShortUrlClick(url)}
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
      </div>
    </>
  );
};

export default UrlsTable;
