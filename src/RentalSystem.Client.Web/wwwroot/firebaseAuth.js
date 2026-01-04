window.firebaseAuth = {
    register: async function (email, password) {
        if (!firebase.apps.length) {
            firebase.initializeApp(window.firebaseConfig);
        }

        try {
            const userCredential = await firebase.auth().createUserWithEmailAndPassword(email, password);
            return await userCredential.user.getIdToken();
        } catch (err) {
            const code = err.code || "unknown_error";
            throw code;
        }
    }
};