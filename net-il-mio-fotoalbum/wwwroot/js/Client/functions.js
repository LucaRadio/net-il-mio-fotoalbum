searchByName();


function searchByName() {
    const inputEl = document.getElementById("input");
    const toSearch = inputEl.value;


    axios.get("/api/Photos",{
        params: {
            search: toSearch
            }
        })
        .then((resp) => {
            const allPhotos = resp.data;
            const rowEl = document.querySelector('.row');
            rowEl.innerHTML = '';
            allPhotos.forEach(singlePhoto => {

                rowEl.innerHTML += `
                                        <div class="col">
                                            <div class="card bg-dark h-100">
                                            <img class="card-img-top" src ="data:image/jpg;base64,${singlePhoto.content}" alt = "Card image cap" >
                                            <div class="card-body d-flex flex-column">
                                                <h5 class="card-title text-white">${singlePhoto.title}</h5>
                                                <p class="card-text text-white flex-grow-1"> ${(singlePhoto.description.length > 50) ? singlePhoto.description.substring(0, 50) + " ..." : singlePhoto.description}</p>
                                                
                                                </div>
                                            </div>
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


    axios.post("api/Messages", messageToSend)

}