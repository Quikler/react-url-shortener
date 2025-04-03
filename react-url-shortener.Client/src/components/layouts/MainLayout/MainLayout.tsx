import { ReactNode } from "react";
import MainHeader from "./MainHeader";
import MainFooter from "./MainFooter";

type MainLayoutProps = {
  children: ReactNode;
}

const MainLayout = ({ children }: MainLayoutProps) => {
  return (
    <div id="main-layout-wrapper" className="dark:text-gray-200 text-gray-800">
      <MainHeader />
      <main id="main" className="dark:bg-gray-800 bg-gray-200">{children}</main>
      <MainFooter />
    </div>
  );
};

export default MainLayout;
