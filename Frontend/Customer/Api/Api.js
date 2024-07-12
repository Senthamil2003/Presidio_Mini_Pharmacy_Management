export async function GetData(url, params, token) {
  try {
    const queryString = new URLSearchParams(params).toString();
    const fullUrl = `${url}?${queryString}`;

    const response = await fetch(fullUrl, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + token,
      },
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error);
    }
    const data = await response.json();
    console.log(data);

    return data;
  } catch (error) {
    console.error("An error occurred while fetching data:", error);
    throw error;
  }
}

export async function PostData(url, params, method, token) {
  try {
    console.log(url, method, params, token);
    const response = await fetch(url, {
      method: method,
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + token,
      },
      body: JSON.stringify(params),
    });

    if (!response.ok) {
      const error = await response.json();
      console.log(error)
      throw new Error(error.message);
    }

    const data = await response.json();
    console.log(data);

    return data;
  } catch (error) {
    console.error("An error occurred while fetching data:", error);
    throw error; // Re-throw the error after logging
  }
}
