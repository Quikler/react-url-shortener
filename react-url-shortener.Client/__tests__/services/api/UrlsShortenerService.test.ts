import { faker } from "@faker-js/faker";
import { UrlRoutes } from "@src/services/api/ApiRoutes";
import { PaginationResponse } from "@src/services/api/models/Shared";
import { UrlInfoResponse, UrlResponse } from "@src/services/api/models/Url";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";
import api from "@src/services/axios/instance";
import MockAdapter from "axios-mock-adapter";

function createUrlResponse(): UrlResponse {
  return {
    id: faker.string.uuid(),
    urlOriginal: faker.internet.url(),
    urlShortened: faker.string.sample(),
    userId: faker.string.uuid(),
  };
}

function createUrlsPagination(count: number, pageNumber: number): PaginationResponse<UrlResponse> {
  const items: UrlResponse[] = [];
  for (let i = 0; i < count; i++) {
    items.push(createUrlResponse());
  }

  return {
    items: items,
    totalCount: faker.number.int(items.length),
    totalPages: faker.number.int(),
    pageNumber: pageNumber,
    pageSize: items.length,
  };
}

function createUrlInfoResponse(): UrlInfoResponse {
  return {
    id: faker.string.uuid(),
    urlOriginal: faker.internet.url(),
    urlShortened: faker.string.sample(),
    user: {
      id: faker.string.uuid(),
      username: faker.internet.username(),
    },
    createdAt: faker.date.anytime().toLocaleString(),
  };
}

describe("UrlsShortenerService", () => {
  let mockAxios: MockAdapter;

  beforeEach(() => {
    // Create a new instance of axios-mock-adapter
    mockAxios = new MockAdapter(api);
  });

  afterEach(() => {
    // Reset the mock adapter after each test
    mockAxios.restore();
  });

  describe("getAll", () => {
    it.each([
      { pageSize: 5, pageNumber: 1 },
      { pageSize: 1, pageNumber: 2 },
      { pageSize: 3, pageNumber: 2 },
    ])("returns COUNT of urls on №Ns page", async ({ pageSize, pageNumber }) => {
      const urlsPagination: PaginationResponse<UrlResponse> = createUrlsPagination(
        pageSize,
        pageNumber
      );

      mockAxios
        .onGet(`${UrlRoutes.base}?pageNumber=${pageNumber}&pageSize=${pageSize}`)
        .reply(200, urlsPagination);

      const result = await UrlsShortenerService.getAll(pageNumber, pageSize);
      expect(result).toEqual(urlsPagination);
    });

    it("should throw on non-cancel error", async () => {
      mockAxios
        .onGet(`${UrlRoutes.base}?pageNumber=${1}&pageSize=${5}`)
        .reply(500, { message: "Error" });

      await expect(UrlsShortenerService.getAll()).rejects.toThrow(
        "Request failed with status code 500"
      );
    });

    it("should not throw on non-cancel error", async () => {
      mockAxios
        .onGet(`${UrlRoutes.base}?pageNumber=${1}&pageSize=${5}`)
        .reply(500, { message: "Error" });

      await expect(UrlsShortenerService.getAll()).rejects.toThrow(
        "Request failed with status code 500"
      );
    });
  });

  describe("getInfo", () => {
    it("gets url info by ID", async () => {
      const urlInfo = createUrlInfoResponse();

      mockAxios.onGet(`${UrlRoutes.base}/${urlInfo.id}`).reply(200, urlInfo);

      const result = await UrlsShortenerService.getInfo(urlInfo.id);
      expect(result).toEqual(urlInfo);
    });

    it("should throw if error is non-cancel error", async () => {
      const urlInfo = createUrlInfoResponse();

      mockAxios.onGet(`${UrlRoutes.base}/${urlInfo.id}`).reply(500, { message: "Error" });

      await expect(UrlsShortenerService.getInfo(urlInfo.id)).rejects.toThrow(
        "Request failed with status code 500"
      );
    });

    it("should not throw if error is cancel error", async () => {
      const abortController = new AbortController();
      const urlInfo = createUrlInfoResponse();

      mockAxios.onGet(`${UrlRoutes.base}/${urlInfo.id}`).reply(() => {
        abortController.abort();
        return [200, { message: "Url Info" }];
      });

      const result = await UrlsShortenerService.getInfo(urlInfo.id, {
        signal: abortController.signal,
      });
      expect(result).toBeUndefined();
    });
  });

  describe("create", () => {
    it("creates url successfully", async () => {
      const url = faker.internet.url();
      const urlResponse = createUrlResponse();
      urlResponse.urlOriginal = url;

      mockAxios.onPost(`${UrlRoutes.base}?url=${url}`).reply(204, urlResponse);

      const result = await UrlsShortenerService.create(url);
      expect(result).toEqual(urlResponse);
    });

    it("should throw if error is non-cancel error", async () => {
      const url = faker.internet.url();

      mockAxios.onPost(`${UrlRoutes.base}?url=${url}`).reply(500, { message: "Error" });
      await expect(UrlsShortenerService.create(url)).rejects.toThrow(
        "Request failed with status code 500"
      );
    });

    it("should not throw if error is cancel error", async () => {
      const abortController = new AbortController();
      const url = faker.internet.url();

      mockAxios.onPost(`${UrlRoutes.base}?url=${url}`).reply(() => {
        abortController.abort();
        return [204, "Url created"];
      });

      const result = await UrlsShortenerService.create(url, { signal: abortController.signal });
      expect(result).toBeUndefined();
    });
  });

  describe("delete", () => {
    it("returns no content and deletes url by ID", async () => {
      const urlId = faker.string.uuid();

      mockAxios.onDelete(`${UrlRoutes.base}/${urlId}`).reply(201);

      const result = await UrlsShortenerService.delete(urlId);
      expect(result).toBeUndefined();
    });

    it("should throw if error is non-cancel error", async () => {
      const urlId = faker.string.uuid();

      mockAxios.onDelete(`${UrlRoutes.base}/${urlId}`).reply(500, { message: "Error" });

      await expect(UrlsShortenerService.delete(urlId)).rejects.toThrow(
        "Request failed with status code 500"
      );
    });

    it("should not throw if error is cancel error", async () => {
      const abortController = new AbortController();
      const urlId = faker.string.uuid();

      mockAxios.onDelete(`${UrlRoutes.base}/${urlId}`).reply(() => {
        abortController.abort();
        return [201];
      });

      const result = await UrlsShortenerService.delete(urlId, { signal: abortController.signal });
      expect(result).toBeUndefined();
    });
  });
});
