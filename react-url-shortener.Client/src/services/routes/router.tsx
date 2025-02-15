import App from "@src/App";
import MainLayout from "@src/components/layouts/MainLayout/MainLayout";
import { AuthProvider } from "@src/hooks/useAuth";
import AboutPage from "@src/pages/about/AboutPage";
import LoginPage from "@src/pages/login/LoginPage";
import SignupPage from "@src/pages/signup/SignupPage";
import UrlsPage from "@src/pages/urls/UrlsPage";
import { createBrowserRouter } from "react-router-dom";

export const router = createBrowserRouter([
  {
    path: "/",
    element: (
      <AuthProvider>
        <MainLayout>
          <App />
        </MainLayout>
      </AuthProvider>
    ),
    children: [
      {
        index: true,
        element: <UrlsPage />,
      },
      {
        path: "urls",
        element: <UrlsPage />,
      },
      {
        path: "login",
        element: <LoginPage />,
      },
      {
        path: "signup",
        element: <SignupPage />,
      },
      {
        path: "about",
        element: <AboutPage />,
      },
    ],
  },
]);
