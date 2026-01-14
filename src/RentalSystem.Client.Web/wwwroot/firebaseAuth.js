window.firebaseAuth = {
    register: async function (email, password) {
        if (!firebase.apps.length) {
            firebase.initializeApp(window.firebaseConfig);
        }

        try {
            const userCredential = await firebase.auth().createUserWithEmailAndPassword(email, password);
            const token = await userCredential.user.getIdToken();
            return { success: true, token: token, error: null };
        } catch (err) {
            return { success: false, token: null, error: err.code || "unknown_error" };
        }
    },

    login: async function (email, password) {
        if (!firebase.apps.length) {
            firebase.initializeApp(window.firebaseConfig);
        }

        try {
            const userCredential = await firebase.auth().signInWithEmailAndPassword(email, password);
            const token = await userCredential.user.getIdToken();
            return { success: true, token: token, error: null };
        } catch (err) {
            return { success: false, token: null, error: err.code || "unknown_error" };
        }
    },

    passwordReset: async function (email) {
        try {
            if (!firebase.apps.length) firebase.initializeApp(window.firebaseConfig);
            await firebase.auth().sendPasswordResetEmail(email);
            return { success: true, error: null };
        } catch (err) {
            return { success: false, error: err.code || "unknown_error" };
        }
    },

    deleteFirebaseAccount: async function () {
        try {
            const user = firebase.auth().currentUser;

            if (!user) {
                console.warn("Delete account: No user found in Firebase state.");
                return "No user logged in";
            }

            await user.delete();
            return "Success";
        } catch (error) {
            console.error("Firebase Delete Error:", error);
            return error.code || error.message;
        }
    }
};