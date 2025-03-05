import { UrlResponse } from "@src/services/api/models/Url";
import { useState } from "react";
import UrlInfoModal from "./UrlInfoModal";
import UrlsTable from "./UrlsTable";

const UrlsTableAndModal = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedUrl, setSelectedUrl] = useState<UrlResponse | null>(null);

  const handleShortUrlClick = (url: UrlResponse) => {
    setIsModalOpen(true);
    setSelectedUrl(url);
  };

  return (
    <>
      <UrlInfoModal
        isModalOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        selectedUrl={selectedUrl}
      />
      <div className="bg-white shadow rounded-2xl border-black">
        <UrlsTable onShortUrlClick={handleShortUrlClick} />
      </div>
    </>
  );
};

export default UrlsTableAndModal;
