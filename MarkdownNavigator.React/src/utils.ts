import { Domain } from "./constants";
import { ResultTextDTO } from "./types";

export const postData = async (url: string, body: any): Promise<ResultTextDTO> => {
  const result = await fetch(
    `${Domain}${url}`, 
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(body),
    })
    .then(response => response.json())
    .catch(error => console.log(error));

    return result;
}