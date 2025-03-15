import api from "@src/services/axios/instance";
import MockAdapter from "axios-mock-adapter";
import { AuthService } from "@src/services/api/AuthService";
import { AuthSuccessResponse, LoginRequest, SignupRequest } from "@src/services/api/models/Auth";
import { AuthRoutes } from "@src/services/api/ApiRoutes";
import { faker } from "@faker-js/faker";

function createSignupRequest(): SignupRequest {
  const password = faker.internet.password();

  return {
    username: faker.internet.username(),
    password: password,
    confirmPassword: password,
  };
}

function createLoginRequest(): LoginRequest {
  return {
    username: faker.internet.username(),
    password: faker.internet.password(),
  };
}

function createAuthSuccessResponse(): AuthSuccessResponse {
  return {
    user: {
      id: faker.string.uuid(),
      username: faker.internet.username(),
    },
    roles: [],
    token: faker.string.sample(),
  };
}

describe("AuthService", () => {
  const authSuccessResponse = createAuthSuccessResponse();

  let mockAxios: MockAdapter;

  beforeEach(() => {
    // Create a new instance of axios-mock-adapter
    mockAxios = new MockAdapter(api);
  });

  afterEach(() => {
    // Reset the mock adapter after each test
    mockAxios.restore();
  });

  describe("signup", () => {
    const signupRequest = createSignupRequest();

    it("returns AuthSuccessResponse on successful signup", async () => {
      mockAxios.onPost(AuthRoutes.signup, signupRequest).reply(200, authSuccessResponse);

      const result = await AuthService.signup(signupRequest);
      expect(result).toEqual(authSuccessResponse);
    });

    it("fails if error was not canceled error", async () => {
      mockAxios.onPost(AuthRoutes.signup, {}).reply(500, { message: "Error" });
      await expect(AuthService.signup({} as SignupRequest)).rejects.toThrow(
        "Request failed with status code 500"
      );
    });

    it("should not fail on cancel error", async () => {
      const abortController = new AbortController();
      mockAxios.onPost(AuthRoutes.signup, signupRequest).reply(() => {
        abortController.abort();
        return [200, { message: "Got" }];
      });

      const result = await AuthService.signup(signupRequest, { signal: abortController.signal });
      expect(result).toBeUndefined();
    });
  });

  describe("login", () => {
    const loginRequest = createLoginRequest();

    it("returns AuthSuccessResponse on successful login", async () => {
      mockAxios.onPost(AuthRoutes.login, loginRequest).reply(200, authSuccessResponse);

      const result = await AuthService.login(loginRequest);
      expect(result).toEqual(authSuccessResponse);
    });

    it("fails if error was not canceled error", async () => {
      mockAxios.onPost(AuthRoutes.login, {}).reply(500, { message: "Error" });
      await expect(AuthService.login({} as LoginRequest)).rejects.toThrow(
        "Request failed with status code 500"
      );
    });

    it("should not fail on cancel error", async () => {
      const abortController = new AbortController();
      mockAxios.onPost(AuthRoutes.login, loginRequest).reply(() => {
        abortController.abort();
        return [200, { message: "Got" }];
      });

      const result = await AuthService.login(loginRequest, { signal: abortController.signal });
      expect(result).toBeUndefined();
    });
  });

  describe("logout", () => {
    it("logouts user successfully", async () => {
      mockAxios.onPost(AuthRoutes.logout).reply(201, { message: "Logout successfully" });

      const result = await AuthService.logout();
      expect(result).toEqual({ message: "Logout successfully" });
    });

    it("fails when error is not cancel error", async () => {
      mockAxios.onPost(AuthRoutes.logout).reply(500, { message: "Error" });
      await expect(AuthService.logout()).rejects.toThrow("Request failed with status code 500");
    });

    it("should not fail on cancel error", async () => {
      const abortController = new AbortController();
      mockAxios.onPost(AuthRoutes.logout).reply(() => {
        abortController.abort();
        return [200, "Logout"];
      });

      const result = await AuthService.logout({ signal: abortController.signal });
      expect(result).toBeUndefined();
    });
  });

  describe("me", () => {
    it("returns AuthSuccessResponse on successful me retrieve", async () => {
      mockAxios.onGet(AuthRoutes.me).reply(200, authSuccessResponse);

      const result = await AuthService.me();
      expect(result).toEqual(authSuccessResponse);
    });

    it("should throw on non-cancel error", async () => {
      mockAxios.onGet(AuthRoutes.me).reply(500, { message: "Error" });

      await expect(AuthService.me()).rejects.toThrow("Request failed with status code 500");
    });

    it("should not throw on cancel error", async () => {
      const abortController = new AbortController();
      mockAxios.onGet(AuthRoutes.me).reply(() => {
        abortController.abort();
        return [200, "Me"];
      });

      const result = await AuthService.me({ signal: abortController.signal });
      expect(result).toBeUndefined();
    });
  });
});
