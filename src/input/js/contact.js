const DestinationAddress = "recitalblooms@gmail.com";
const FormUrl = "https://commands.brigh.id/commands/email/execute/recaptcha";

const submit = document.getElementById("submit");
const form = document.getElementById("contact-form");
const formState = { token: "", email: "", company: "", phone: "", message: "", name: "" };

function onRecaptchaSubmit(token) {
    formState.token = token;
    submit.removeAttribute("disabled");
}

function onRecaptchaExpired() {
    formState.token = "";
    submit.setAttribute("disabled", "disabled");
}

function getInputValue(id) {
    const input = document.getElementById(id)
    return input.value;
}

function readFormDataIntoState() {
    formState.email = getInputValue("email-address");
    formState.company = getInputValue("company");
    formState.phone = getInputValue("phone");
    formState.message = getInputValue("message");
    formState.name = getInputValue("name");
}

function showThanksMessage() {
    const form = document.querySelector("form");
    form.innerHTML = `<div class="thanks"><h2>Thanks for Contacting Us!</h2><p>We will reply as soon as possible.</p></div>`;
}

form.onsubmit = async function (event) {
    event.preventDefault();
    readFormDataIntoState();

    const response = await fetch(FormUrl, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "G-Recaptcha-Response": formState.token,
        },
        body: JSON.stringify({
            To: DestinationAddress,
            From: formState.email,
            Subject: "Recital Blooms Contact Form Submission",
            Message: `New Recital Blooms Contact Request:\n\nName: ${formState.name}\nEmail: ${formState.email}\nCompany: ${formState.company}\nPhone: ${formState.phone}\nMessage: ${formState.message}`,
        })
    });

    if (response.status === 200) {
        showThanksMessage();
    }
}

// showThanksMessage();