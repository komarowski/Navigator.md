import { useState, useEffect } from "react";
import { Domain } from "./constants";


/**
 * Hook for data fetching with GET request.
 * @param url Relative request url.
 * @param initialState Initial state.
 * @param deps String whose change triggers the hook call.
 * @param isIgnoreFetch Condition to skip the fetch request.
 * @returns Response or Initial state.
 */
export function useGetFetch<T>(
  url: string, 
  initialState: T,
  deps: string,
  isIgnoreFetch: boolean = false
): T {
  const [result, setResult] = useState(initialState);
  useEffect(() => {
    let ignore = false;
    if (!isIgnoreFetch){
      //console.log({ Request: "GET " + url});
      fetch(`${Domain}${url}`)
      .then(response => response.json())
      .then(responseResult => {
        if (!ignore) {
          setResult(responseResult);
        }
      })
      .catch(error => console.log(error));
    } else {
      setResult(initialState);
    } 
    return () => {
      ignore = true;
    };
  }, [deps]);
  return result;
};

export function usePostFetch<T>(
  url: string,
  initialState: T,
  body: any,
  deps: string,
  isIgnoreFetch: boolean = false
): T {
  const [result, setResult] = useState(initialState);

  useEffect(() => {
    let ignore = false;
    if (!isIgnoreFetch) {
      //console.log({ Request: "POST " + url});
      fetch(`${Domain}${url}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(body),
      })
        .then(response => response.json())
        .then(responseResult => {
          if (!ignore) {
            setResult(responseResult);
          }
        })
        .catch(error => console.log(error));
    } else {
      setResult(initialState);
    }
    return () => {
      ignore = true;
    };
  }, [deps]);

  return result;
}


/**
 * Hook to set a delay time until a user stops typing to change state.
 * @param defaultValue default value.
 * @param delay delay in milliseconds.
 * @returns useState() hook with debouncing.
 */
export function useTextDebounce(defaultValue: string, delay = 350): [string, React.Dispatch<React.SetStateAction<string>>] {
  const [text, setText] = useState(defaultValue);
  const [tempText, setTempText] = useState(defaultValue);

  useEffect(() => {
    const timeoutID = setTimeout(() => setText(tempText), delay);
    return () => clearTimeout(timeoutID);
  }, [tempText, delay]);

  return [text, setTempText];
};

/**
 * Get value from local storage.
 * @param key local storage key.
 * @param defaultValue default value.
 * @returns local storage value.
 */
function getStorageValue(key: string, defaultValue: any): any {
  const saved = localStorage.getItem(key);
  const initial = saved && JSON.parse(saved);
  return initial || defaultValue;
}

/**
 * Hook for saving state in local storage.
 * @param key local storage key.
 * @param defaultValue default value.
 * @returns useState() hook.
 */
export function useLocalStorage<T>(key: string, defaultValue: T): [T, React.Dispatch<React.SetStateAction<T>>]{
  const [value, setValue] = useState<T>(() => {
    return getStorageValue(key, defaultValue);
  });

  useEffect(() => {
    localStorage.setItem(key, JSON.stringify(value));
  }, [key, value]);

  return [value, setValue];
};