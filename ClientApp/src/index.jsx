// import 'bootstrap/dist/css/bootstrap.min.css';
// import 'bootstrap-icons/font/bootstrap-icons.css'; // Optional if you want to use icons later
// import React from 'react';
// import ReactDOM from 'react-dom/client';
// import { Carousel } from 'react-bootstrap';
// import './app.css'; // Make sure this file exists

// const App = () => {
//   const speak = (text) => {
//     const utterance = new SpeechSynthesisUtterance(text);
//     utterance.lang = 'en-US';
//     const voices = window.speechSynthesis.getVoices();
//     const selectedVoice = voices.find(v => v.name.includes("Google") || v.lang === "en-US");
//     if (selectedVoice) utterance.voice = selectedVoice;
//     window.speechSynthesis.speak(utterance);
//   };

//   const startListening = () => {
//     const recognition = new window.webkitSpeechRecognition();
//     recognition.lang = 'en-US';
//     recognition.onresult = (event) => {
//       const command = event.results[0][0].transcript;
//       alert("You said: " + command);

//       if (command.toLowerCase().includes("add")) {
//         const product = command.toLowerCase().replace("add", "").trim();
//         const existingCart = JSON.parse(localStorage.getItem("offline_cart") || "[]");
//         existingCart.push(product);
//         localStorage.setItem("offline_cart", JSON.stringify(existingCart));
//         speak(`${product} has been added to your offline cart`);
//       }
//     };
//     recognition.start();
//   };

//   React.useEffect(() => {
//     speak("Welcome to Mandy's. Your one-stop eCommerce site created just for you.");
//   }, []);

//   return (
//     <>
//       {/* Carousel Section */}
//       <div className="carousel-wrapper">
//         <Carousel fade controls interval={5000} pause={false}>
//           <Carousel.Item>
//             <img
//               className="d-block w-100"
//               src="/images/perfumewoman.png"
//               alt="Braille keyboard for visually impaired users"
//             />
//             <Carousel.Caption>
//               <h1>Premium Assistive Devices</h1>
//               <p>Empowering independence for the visually impaired.</p>
             
//             </Carousel.Caption>
//           </Carousel.Item>

//           <Carousel.Item>
//             <img
//               className="d-block w-100"
//               src="/images/Talkingwatchtwo.png"
//               alt="Talking watch for the blind"
//             />
//             <Carousel.Caption>
//               <h1>Innovative Wearables</h1>
//               <p>Enhance daily living with smart assistive gadgets.</p>
             
//             </Carousel.Caption>
//           </Carousel.Item>

//           <Carousel.Item>
//             <img
//               className="d-block w-100"
//               src="/images/Whitecanetwo.png"
//               alt="White cane for visually impaired navigation"
//             />
//             <Carousel.Caption>
//               <h1>Trusted Mobility Tools</h1>
//               <p>Navigate confidently with our white canes.</p>
             
//             </Carousel.Caption>
//           </Carousel.Item>
//         </Carousel>

        
//       </div>

//       {/* CTA Section */}
//       <div className="text-center p-5 bg-light mt-5">
//         <h2>Shop Smarter. Shop Inclusive.</h2>
//         <p className="lead">Discover AI-powered assistive devices designed for everyone.</p>
//         <a href="/Shop/Index" className="btn btn-success">Visit</a>
//       </div>
//     </>
//   );
// };

// console.log("React bundle is working!");
// const root = ReactDOM.createRoot(document.getElementById('react-root'));
// root.render(<App />);






// // Import required styles
// import 'bootstrap/dist/css/bootstrap.min.css';
// import 'bootstrap-icons/font/bootstrap-icons.css';
// import React from 'react';
// import ReactDOM from 'react-dom/client';
// import { Carousel } from 'react-bootstrap';
// import './app.css'; // Ensure this file exists

// const App = () => {
//   const speak = (text) => {
//     const utterance = new SpeechSynthesisUtterance(text);
//     utterance.lang = 'en-US';

//     const voices = window.speechSynthesis.getVoices();
//     const selectedVoice = voices.find(v => v.name.includes("Google") || v.lang === "en-US");
//     if (selectedVoice) {
//       utterance.voice = selectedVoice;
//     }
//     window.speechSynthesis.speak(utterance);
//   };

//   const startVoiceRecognition = () => {
//     if (!('webkitSpeechRecognition' in window)) {
//       alert("Speech recognition not supported in this browser.");
//       return;
//     }

//     const recognition = new window.webkitSpeechRecognition();
//     recognition.lang = 'en-US';
//     recognition.continuous = true;
//     recognition.interimResults = false;

//     recognition.onresult = (event) => {
//       const command = event.results[event.results.length - 1][0].transcript.toLowerCase().trim();
//       console.log("You said:", command);

//       if (command.includes("add")) {
//         const product = command.replace("add", "").trim();
//         const existingCart = JSON.parse(localStorage.getItem("offline_cart") || "[]");
//         existingCart.push(product);
//         localStorage.setItem("offline_cart", JSON.stringify(existingCart));
//         speak(`${product} has been added to your offline cart`);
//       } else if (command.includes("chatbot")) {
//         speak("Navigating to chatbot");
//         window.location.href = "/Chatbot/Index";
//       } else if (command.includes("shop")) {
//         speak("Navigating to shop");
//         window.location.href = "/Shop/Index";
//       } else {
//         speak("Sorry, I didn't understand.");
//       }
//     };

//     recognition.onerror = (e) => {
//       console.error("Speech recognition error:", e.error);
//     };

//     recognition.onend = () => {
//       console.log("Restarting recognition...");
//       recognition.start(); // Keep listening continuously
//     };

//     recognition.start();
//     speak("Voice assistant started. You can say add followed by product name, shop, or chatbot.");
//   };

//   // Start speech after voices load and after user interaction
//   React.useEffect(() => {
//     let initialized = false;

//     const initialize = () => {
//       if (!initialized) {
//         speechSynthesis.onvoiceschanged = () => {
//           speak("Welcome to Mandy's. Your one-stop eCommerce site created just for you.");
//         };
//         startVoiceRecognition();
//         initialized = true;
//       }
//     };

//     window.addEventListener('click', initialize);
//     window.addEventListener('keydown', initialize);

//     return () => {
//       window.removeEventListener('click', initialize);
//       window.removeEventListener('keydown', initialize);
//     };
//   }, []);

//   return (
//     <>
//       {/* Carousel Section */}
//       <div className="carousel-wrapper">
//         <Carousel fade controls interval={5000} pause={false}>
//           <Carousel.Item>
//             <img
//               className="d-block w-100"
//               src="/images/perfumewoman.png"
//               alt="Braille keyboard for visually impaired users"
//             />
//             <Carousel.Caption>
//               <h1>Premium Assistive Devices</h1>
//               <p>Empowering independence for the visually impaired.</p>
//             </Carousel.Caption>
//           </Carousel.Item>

//           <Carousel.Item>
//             <img
//               className="d-block w-100"
//               src="/images/Talkingwatchtwo.png"
//               alt="Talking watch for the blind"
//             />
//             <Carousel.Caption>
//               <h1>Innovative Wearables</h1>
//               <p>Enhance daily living with smart assistive gadgets.</p>
//             </Carousel.Caption>
//           </Carousel.Item>

//           <Carousel.Item>
//             <img
//               className="d-block w-100"
//               src="/images/Whitecanetwo.png"
//               alt="White cane for visually impaired navigation"
//             />
//             <Carousel.Caption>
//               <h1>Trusted Mobility Tools</h1>
//               <p>Navigate confidently with our white canes.</p>
//             </Carousel.Caption>
//           </Carousel.Item>
//         </Carousel>
//       </div>

//       {/* CTA Section */}
//       <div className="text-center p-5 bg-light mt-5">
//         <h2>Shop Smarter. Shop Inclusive.</h2>
//         <p className="lead">Discover AI-powered assistive devices designed for everyone.</p>
//         <a href="/Shop/Index" className="btn btn-success">Visit</a>
//       </div>
//     </>
//   );
// };



// const root = ReactDOM.createRoot(document.getElementById('react-root'));
// root.render(<App />);








// Import required styles
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import React, { useEffect } from 'react';
import ReactDOM from 'react-dom/client';
import { Carousel } from 'react-bootstrap';
import './app.css'; // Ensure this file exists


const App = () => {
  const speak = (text) => {
    const utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = 'en-US';

    const voices = window.speechSynthesis.getVoices();
    const selectedVoice = voices.find(v => v.name.includes("Google") || v.lang === "en-US");
    if (selectedVoice) {
      utterance.voice = selectedVoice;
    }
    window.speechSynthesis.speak(utterance);
  };

  const startVoiceRecognition = () => {
    if (!('webkitSpeechRecognition' in window)) {
      alert("Speech recognition not supported in this browser.");
      return;
    }

    const recognition = new window.webkitSpeechRecognition();
    recognition.lang = 'en-US';
    recognition.continuous = true;
    recognition.interimResults = false;

    let lastCommand = "";
    let saidSorry = false;

    recognition.onresult = (event) => {
      const command = event.results[event.results.length - 1][0].transcript.toLowerCase().trim();
      console.log("You said:", command);

      if (!command) return;

      if (command !== lastCommand) {
        lastCommand = command;
        saidSorry = false;
      }

      if (command.includes("add")) {
        saidSorry = false;
        const product = command.replace("add", "").trim();
        const existingCart = JSON.parse(localStorage.getItem("offline_cart") || "[]");
        existingCart.push(product);
        localStorage.setItem("offline_cart", JSON.stringify(existingCart));
        speak(`${product} has been added to your offline cart`);
      } else if (command.includes("chatbot")) {
        saidSorry = false;
        speak("Navigating to chatbot");
        window.location.href = "/Chatbot/Index";
      } else if (command.includes("shop")) {
        saidSorry = false;
        speak("Navigating to shop");
        window.location.href = "/Shop/Index";
      } else {
        if (!saidSorry) {
          speak("Sorry, I didn't understand.");
          saidSorry = true;
        }
      }
    };

    recognition.onerror = (e) => {
      console.error("Speech recognition error:", e.error);
    };

    recognition.onend = () => {
      console.log("Restarting recognition...");
      setTimeout(() => recognition.start(), 1000);
    };

    recognition.start();
    speak("Voice assistant started. You can say add followed by product name, shop, or chatbot.");
  };

  // Start speech recognition after first user interaction, no welcome message
  useEffect(() => {
    let initialized = false;

    const initialize = () => {
      if (!initialized) {
        startVoiceRecognition();
        initialized = true;
      }
    };

    window.addEventListener('click', initialize);
    window.addEventListener('keydown', initialize);

    return () => {
      window.removeEventListener('click', initialize);
      window.removeEventListener('keydown', initialize);
    };
  }, []);

  return (
    <>
      {/* Carousel Section */}
      <div className="carousel-wrapper">
        <Carousel fade controls interval={5000} pause={false}>
          <Carousel.Item>
            <img
              className="d-block w-100"
              src="/images/perfumewoman.png"
              alt="Braille keyboard for visually impaired users"
            />
            <Carousel.Caption>
              <h1>Premium Assistive Devices</h1>
              <p>Empowering independence for the visually impaired.</p>
            </Carousel.Caption>
          </Carousel.Item>

          <Carousel.Item>
            <img
              className="d-block w-100"
              src="/images/Talkingwatchtwo.png"
              alt="Talking watch for the blind"
            />
            <Carousel.Caption>
              <h1>Innovative Wearables</h1>
              <p>Enhance daily living with smart assistive gadgets.</p>
            </Carousel.Caption>
          </Carousel.Item>

          <Carousel.Item>
            <img
              className="d-block w-100"
              src="/images/Whitecanetwo.png"
              alt="White cane for visually impaired navigation"
            />
            <Carousel.Caption>
              <h1>Trusted Mobility Tools</h1>
              <p>Navigate confidently with our white canes.</p>
            </Carousel.Caption>
          </Carousel.Item>
        </Carousel>
      </div>

      {/* CTA Section */}
      <div className="text-center p-5 bg-light mt-5">
        <h2>Shop Smarter. Shop Inclusive.</h2>
        <p className="lead">Discover AI-powered assistive devices designed for everyone.</p>
        <a href="/Shop/Index" className="btn btn-success">Visit</a>
      </div>
    </>
  );
};

const root = ReactDOM.createRoot(document.getElementById('react-root'));
root.render(<App />);












