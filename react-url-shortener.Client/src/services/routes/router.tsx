import App from "@src/App";
import MainLayout from "@src/components/layouts/MainLayout/MainLayout";
import { AuthProvider } from "@src/hooks/useAuth";
import AboutPage from "@src/pages/about/AboutPage";
import LoginPage from "@src/pages/login/LoginPage";
import SignupPage from "@src/pages/signup/SignupPage";
import UrlsMain from "@src/pages/urls/UrlsMain";
import UrlsPage from "@src/pages/urls/UrlsPage";
import UrlInfoPage from "@src/pages/urls/{urlId}/UrlInfoPage";
import { createBrowserRouter } from "react-router-dom";
import AuthRoute from "./AuthRoute";
import { ToastContextProvider } from "@src/hooks/useToast";

export const router = createBrowserRouter([
  {
    path: "/",
    element: (
      <AuthProvider>
        <MainLayout>
          <ToastContextProvider>
            <App />
          </ToastContextProvider>
        </MainLayout>
      </AuthProvider>
    ),
    children: [
      {
        path: "urls",
        element: <UrlsPage />,
        children: [
          {
            index: true,
            element: <UrlsMain />,
          },
          {
            path: ":urlId/info",
            element: (
              <AuthRoute>
                <UrlInfoPage />
              </AuthRoute>
            ),
          },
        ],
      },
      {
        path: "urls",
        element: <UrlsPage />,
        children: [
          { path: "", index: true, element: <UrlsMain /> },
          {
            path: ":urlId/info",
            element: (
              <AuthRoute>
                <UrlInfoPage />
              </AuthRoute>
            ),
          },
        ],
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
        index: true,
        element: <AboutPage />,
      },
    ],
  },
]);
