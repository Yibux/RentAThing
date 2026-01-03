window.initFirebase = function () {
    if (!firebase.apps.length) {
        firebase.initializeApp(window.firebaseConfig);
    }
};
