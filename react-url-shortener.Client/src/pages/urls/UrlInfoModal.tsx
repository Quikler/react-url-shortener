import Button from "@src/components/ui/Buttons/Button";
import ButtonLink from "@src/components/ui/Buttons/ButtonLink";
import Modal from "@src/components/ui/Modal";
import { useAuth } from "@src/hooks/useAuth";
import { UrlResponse } from "@src/services/api/models/Url";

type UrlInfoModalProps = {
  isModalOpen: boolean;
  onClose: () => void;
  selectedUrl: UrlResponse | null;
};

const UrlInfoModal = ({ isModalOpen, onClose, selectedUrl }: UrlInfoModalProps) => {
  if (!selectedUrl) return null;
	
	const { isUserLoggedIn } = useAuth();

  const handleCopy = () => navigator.clipboard.writeText(selectedUrl.urlShortened);

  return (
    <Modal title="Choose an option" isOpen={isModalOpen} onClose={onClose}>
      <span className="block text-xl">Url:</span> {selectedUrl?.urlShortened}
      <div className="mt-4 flex gap-2">
        <ButtonLink to={(selectedUrl && selectedUrl?.urlShortened) || "#"} target="_blank">
          Redirect
        </ButtonLink>
        {isUserLoggedIn() && (
          <ButtonLink to={`/urls/${selectedUrl?.id}/info`} variant="info">
            Info
          </ButtonLink>
        )}
        <Button onClick={handleCopy} variant="secondary">
          Copy
        </Button>
      </div>
    </Modal>
  );
};

export default UrlInfoModal;
