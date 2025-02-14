import axios from "axios";

export const throwIfErrorNotCancelError = (error: any) => {
    if (!axios.isCancel(error)) {
      throw error;
    }
  };