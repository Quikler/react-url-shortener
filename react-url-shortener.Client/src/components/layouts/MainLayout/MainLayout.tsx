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
      <main id="main">{children}</main>
      <MainFooter />
    </>
  );
};

export default MainLayout;
