import UrlsTable from "./UrlsTable";

const UrlsPage = () => {
  return (
    <div className="flex flex-col">
      <div className=" overflow-x-auto">
        <div className="min-w-full inline-block align-middle">
          <div className="flex justify-center py-24 border rounded-lg border-gray-300">
            <UrlsTable />
          </div>
        </div>
      </div>
    </div>
  );
};

export default UrlsPage;
