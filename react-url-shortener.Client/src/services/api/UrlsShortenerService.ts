import { throwIfErrorNotCancelError } from "@src/utils/helpers";
import { AxiosRequestConfig } from "axios";
import api from "@services/axios/instance";
import { UrlRoutes } from "./ApiRoutes";

export abstract class UrlsShortenerService {
  static async getAll(config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.get(UrlRoutes.base, config);
      return response.data;
    } catch (e: any) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async getInfo(urlId: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.get(`${UrlRoutes.base}/${urlId}`, config);
      return response.data;
    } catch (e: any) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async create(url: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.post(`${UrlRoutes.base}?url=${url}`, {}, config);
      return response.data;
    } catch (e: any) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async delete(urlId: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.delete(`${UrlRoutes.base}/${urlId}`, config);
      return response.data;
    } catch (e: any) {
      throwIfErrorNotCancelError(e);
    }
  }
}
