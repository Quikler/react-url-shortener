import { Outlet } from "react-router-dom";

const UrlsPage = () => {
  return (
    <div className="flex flex-col">
      <div className=" w-full mx-auto inline-block align-middle">
        <div className="flex flex-col gap-6 items-center p-6 my-24 border rounded-lg border-gray-300">
          <Outlet />
        </div>
      </div>
    </div>
  );
};

export default UrlsPage;
