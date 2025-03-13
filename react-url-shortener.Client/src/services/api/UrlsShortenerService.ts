import { throwIfErrorNotCancelError } from "@src/utils/helpers";
import { AxiosRequestConfig } from "axios";
import api from "@src/services/axios/instance";
import { UrlRoutes } from "./ApiRoutes";
import { UrlResponse } from "@src/services/api/models/Url";

export abstract class UrlsShortenerService {
  static async getAll(
    pageNumber: number = 5,
    pageSize: number = 5,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.get(
        `${UrlRoutes.base}?pageNumber=${pageNumber}&pageSize=${pageSize}`,
        config
      );
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

  static async create(
    url: string,
    config?: AxiosRequestConfig<any> | undefined
  ): Promise<UrlResponse | undefined> {
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
