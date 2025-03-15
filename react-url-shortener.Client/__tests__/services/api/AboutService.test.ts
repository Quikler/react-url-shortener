import api from "@src/services/axios/instance";
import MockAdapter from "axios-mock-adapter";
import { AboutService } from "@src/services/api/AboutService";

describe("AboutService", () => {
  let mockAxios: MockAdapter;

  beforeEach(() => {
    // Create a new instance of axios-mock-adapter
    mockAxios = new MockAdapter(api);
  });

  afterEach(() => {
    // Reset the mock adapter after each test
    mockAxios.restore();
  });

  // Test 1: getAbout - Success
  it("getAbout returns data on success", async () => {
    const mockData = "This is the about content.";

    // Mock a successful GET request to /about
    mockAxios.onGet("/about").reply(200, mockData);

    const result = await AboutService.getAbout();
    expect(result).toEqual(mockData);
  });

  // Test 2: getAbout - Error (non-cancel error)
  it("getAbout throws an error on non-cancel error", async () => {
    // Mock a failed GET request to /about
    mockAxios.onGet("/about").reply(500, { message: "Internal Server Error" });
    await expect(AboutService.getAbout()).rejects.toThrow("Request failed with status code 500");
  });

  // Test 3: getAbout - Cancel error (should not throw)
  it("getAbout does not throw on cancel error", async () => {
    // Mock a canceled GET request to /about
    const abortController = new AbortController();

    mockAxios.onGet("/about").reply(() => {
      // Simulate a request that will be canceled
      abortController.abort();
      return [200, { message: "Got" }];
    });

    const result = await AboutService.getAbout({ signal: abortController.signal });
    expect(result).toBeUndefined();
  });

  // Test 4: updateAbout - Success
  it("updateAbout returns data on success", async () => {
    const newAbout = "Updated about content";
    const mockData = { about: newAbout };

    // Mock a successful PUT request to /about
    mockAxios.onPut(`/about?newAbout=${encodeURIComponent(newAbout)}`).reply(200, mockData);

    const result = await AboutService.updateAbout(newAbout);
    expect(result).toEqual(mockData);
  });

  // Test 5: updateAbout - Error (non-cancel error)
  it("updateAbout throws an error on non-cancel error", async () => {
    const newAbout = "Updated about content";

    // Mock a failed PUT request to /about
    mockAxios
      .onPut(`/about?newAbout=${encodeURIComponent(newAbout)}`)
      .reply(400, { message: "Bad Request" });

    await expect(AboutService.updateAbout(newAbout)).rejects.toThrow(
      "Request failed with status code 400"
    );
  });

  // Test 6: updateAbout - Cancel error (should not throw)
  it("updateAbout does not throw on cancel error", async () => {
    const newAbout = "Updated about content";

    const abortController = new AbortController();

    // Mock the PUT request with the signal from the AbortController
    mockAxios.onPut(`/about?newAbout=${encodeURIComponent(newAbout)}`).reply(() => {
      // Simulate a request that will be canceled
      abortController.abort();
      return [200, { message: "Updated" }];
    });

    const result = await AboutService.updateAbout(newAbout, { signal: abortController.signal });
    expect(result).toBeUndefined();
  });
});
