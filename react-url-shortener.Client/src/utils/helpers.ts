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

/**
 * Func to get variant style
 * @constructor
 * @param {string} variants - The variants array to go through.
 * @param {string} variant - The that should be taken variant.
 * @param {string} defaultStyle - The default style to which taken variant will be concatenated
 * @returns {string} The string style representation that can be used in className of component
 */
function getVariantStyle<T extends { variant?: string }>(
  variants: { key: T["variant"]; style: string }[],
  variant: string,
  defaultStyle: string
): string {
  for (const v of variants) {
    if (v.key == variant) {
      return [defaultStyle, v.style].join(" ").trim();
    }
  }

  return "";
}

const getVariant = (
  variants: {
    key: string | undefined;
    style: string;
  }[],
  variant: string,
  defaultStyle: string
) => {
  return getVariantStyle(variants, variant, defaultStyle);
};

export default getVariant;


export function generateUUID() { // Public Domain/MIT
  var d = new Date().getTime();//Timestamp
  var d2 = ((typeof performance !== 'undefined') && performance.now && (performance.now() * 1000)) || 0;//Time in microseconds since page-load or 0 if unsupported
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    var r = Math.random() * 16;//random number between 0 and 16
    if (d > 0) {//Use timestamp until depleted
      r = (d + r) % 16 | 0;
      d = Math.floor(d / 16);
    } else {//Use microseconds since page-load if supported
      r = (d2 + r) % 16 | 0;
      d2 = Math.floor(d2 / 16);
    }
    return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
  });
}
