import AuthComponent from "@src/components/HOC/AuthComponent";
import AddUrlSection from "./AddUrlSection";
import UrlsTableAndModal from "./UrlsTableAndModal";
import { UrlsContextProvider } from "./UrlsContext";

const UrlsMain = () => {
  return (
    <UrlsContextProvider>
      <AuthComponent>
        <AddUrlSection />
      </AuthComponent>
      <UrlsTableAndModal />
    </UrlsContextProvider>
  );
};

export default UrlsMain;
