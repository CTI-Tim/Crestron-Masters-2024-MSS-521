var path = location.pathname;
var url = "";

var submitBtn = document.getElementById("sendDriver");

submitBtn.addEventListener("click", (e) => {
  console.log("Submit Pressed");
  SendDriverDetails();
});

async function SendDriverDetails() {
  var input = document.querySelector('input[type="file"]');
  var address = document.getElementById("driverIP").value;
  console.log(address);

  var backup = {
    FileName: input.files[0].name,
  };
  let data = JSON.stringify(backup);
  console.log(data);

  if (input.files[0].name !== null) {
    try {
      if (path.indexOf("VirtualControl") !== -1) {
        url = path.replace(/([^\/]*\/)$/, "") + "cws/api/drivers/";
      } else {
        url = "/cws/api/drivers/";
      }

      console.log(url);
      let result = await fetch(url, {
        method: "POST",
        body: input.files[0],
        headers: {
          // Add the file name as a custom header
          "X-File-Name": input.files[0].name,
          "X-File-IP": address,
        },
      });
      let r = await result.json();
      console.log(r);
    } catch (error) {
      console.log("SERVER UNAVAILABLE : " + error);
    }
  }
}
