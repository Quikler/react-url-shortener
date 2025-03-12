import type { Config } from "jest";

const config: Config = {
  testEnvironment: "jsdom",
  setupFilesAfterEnv: ["<rootDir>/setup-tests.ts"],
  moduleNameMapper: {
    "\\.(css|less|scss|sass)$": "identity-obj-proxy",
    "\\.(jpg|jpeg|png|gif|webp|svg)$": "<rootDir>/__mocks__/fileMock.js",
    "^@src(.*)$": "<rootDir>/src$1",
    "^@components(.*)$": "<rootDir>/src/components$1",
  },
  collectCoverageFrom: [
    "src/**/*.{js,jsx,ts,tsx}",
    "!src/**/*.d.ts",
    "!src/**/*.{spec,test}.{js,jsx,ts,tsx}",
    "!**/node_modules/**",
    "!**/vendor/**",
    "!**/dist/**",
    "!**/build/**",
    "!vite.config.ts",
    "!**/coverage/**",
  ],
  coveragePathIgnorePatterns: ["/node_modules/", "setup-tests.ts", "vite-env.d.ts"],
  transform: {
    "^.+\\.tsx?$": [
      "ts-jest",
      {
        tsConfig: "tsconfig.test.json",
      },
    ],
  },
};

export default config;
