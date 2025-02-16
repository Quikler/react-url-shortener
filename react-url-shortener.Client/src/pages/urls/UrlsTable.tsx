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

type UrlsTableProps = {
  urls: UrlResponse[];
  onUrlDeleted: (urlId: string) => void;
};

const UrlsTable = ({ urls, onUrlDeleted }: UrlsTableProps) => {
  const { user, hasRole } = useAuth();

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedUrl, setSelectedUrl] = useState<UrlResponse | null>(null);

  const handleDeleteUrl = async (urlId: string) => {
    try {
      await UrlsShortenerService.delete(urlId);
      onUrlDeleted(urlId);
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
            <ButtonLink to={`/urls/${selectedUrl?.id}/info`} variant="info">
              Info
            </ButtonLink>
            <Button
              onClick={() => selectedUrl && navigator.clipboard.writeText(selectedUrl.urlShortened)}
              variant="secondary"
            >
              Copy
            </Button>
          </div>
        </div>
      </Modal>
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
                    <button
                      //to={url.urlShortened}
                      //target="_blank"
                      onClick={() => handleShortUrlClick(url)}
                      className="px-2 py-1 text-blue-400 hover:text-blue-500 rounded"
                    >
                      {content}
                    </button>
                  );

                  if (url.userId === user?.id || hasRole(Roles.Admin)) {
                    return (
                      <div className="flex justify-between">
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
    </>
  );
};

export default UrlsTable;
