import { AxiosRequestConfig } from "axios";
import api from "../axios/instance";
import { throwIfErrorNotCancelError } from "@src/utils/helpers";

export abstract class AboutService {
  public static async getAbout(config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.get("/about", config);
      return response.data;
    } catch (e: any) {
      throwIfErrorNotCancelError(e);
    }
  }

  public static async updateAbout(newAbout: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.put(`/about?newAbout=${newAbout}`, null, config);
      return response.data;
    } catch (e: any) {
      throwIfErrorNotCancelError(e);
    }
  }
}
