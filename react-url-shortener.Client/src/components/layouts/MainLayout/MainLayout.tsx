import { ReactNode } from "react";
import MainHeader from "./MainHeader";
import MainFooter from "./MainFooter";

type MainLayoutProps = {
  children: ReactNode;
}

const MainLayout = ({ children }: MainLayoutProps) => {
  return (
    <>
      <MainHeader />
      <main id="main" className="dark:bg-gray-800 bg-white">{children}</main>
      <MainFooter />
    </>
  );
};

export default MainLayout;
