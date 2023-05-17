searchByName();


function searchByName() {
    const inputEl = document.getElementById("input");
    const toSearch = inputEl.value;


    axios.get("/api/Photos/Photos", {
        params: {
            search: toSearch
        }
    })
        .then((resp) => {
            console.log(resp.data);
            const allPhotos = resp.data;
            const rowEl = document.querySelector('.row');
            rowEl.innerHTML = '';
            allPhotos.forEach(singlePhoto => {

                rowEl.innerHTML += `
                                        <div class="col">
                                            <a href="${window.location.origin + '/Photos/Show/' + singlePhoto.id}" class="card text-decoration-none bg-dark h-100">
                                            <img class="card-img-top" src ="data:image/jpg;base64,${singlePhoto.content}">
                                            <div class="card-body d-flex flex-column">
                                                <h5 class="card-title text-white">${singlePhoto.title}</h5>
                                                <p class="card-text text-white flex-grow-1"> ${(singlePhoto.description.length > 50) ? singlePhoto.description.substring(0, 50) + " ..." : singlePhoto.description}</p>
                                                
                                                </div>
                                            </a>
                                            </div>
                                            `

            })

        })
        .catch(err => console.log(err))
}


function sendMessage() {
    const emailEl = document.querySelector("[type='email']");
    const textAreaEl = document.querySelector("textarea");


    const messageToSend = {

        "emailSender": emailEl.value,
        "messageContent": textAreaEl.value
    }

    axios.post(window.location.origin + "/api/Messages/Create", messageToSend,
        {
            headers: { 'Content-Type': 'application/json' },
        })
        .then(resp => {
            const containerEl = document.querySelector(".container");
            containerEl.innerHTML += `<div style="position: absolute; top: 0; right: 0;" class="p-3">
                                     <div class="toast d-block bg-success text-white" role="alert" data-autohide="true" aria-live="assertive" aria-atomic="true">
                                      <div class="toast-header d-flex">
                                        <strong class="flex-grow-1">Message sended</strong>
                                      </div>
                                      <div class="toast-body">
                                        Message was correctly sender. Admin will answer ASAP!
                                      </div>
                                    </div>
                                    </div>`;
            containerEl.append(toastDivEl);



        })
        .catch(error => {
            const containerEl = document.querySelector(".container");
            containerEl.innerHTML += `<div style="position: absolute; top: 0; right: 0;" class="p-3">
                                     <div class="toast d-block bg-danger text-white" role="alert" data-autohide="true" aria-live="assertive" aria-atomic="true">
                                      <div class="toast-header d-flex">
                                        <strong class="flex-grow-1">Message failure</strong>
                                      </div>
                                      <div class="toast-body">
                                        Message was not correctly sender. Retry!
                                      </div>
                                    </div>
                                    </div>`;
            containerEl.append(toastDivEl);
        })
            setTimeout(() => {
                const toast = document.querySelector(".toast");
                toast.classList.remove("d-block");
            },3000)


}



