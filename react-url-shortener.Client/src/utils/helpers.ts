import axios from "axios";

export const throwIfErrorNotCancelError = (error: any) => {
  if (!axios.isCancel(error)) {
    throw error;
  }
};

export const handleError = (error: any, errorFunc: (message: string) => void) => {
  if (axios.isAxiosError(error)) {
    var err = error.response;
    if (Array.isArray(err?.data.errors)) {
      for (let val of err?.data.errors) {
        errorFunc(val);
      }
    } else if (typeof err?.data.errors === "object") {
      for (let e in err?.data.errors) {
        errorFunc(err?.data.errors[e][0]);
      }
    } else if (err?.data) {
      errorFunc(err?.data);
    } else if (err?.status == 401) {
      errorFunc("Please login");
      //window.history.pushState({}, "LoginPage", "/login");
    } else if (err) {
      errorFunc(err?.data);
    }
  }
};
