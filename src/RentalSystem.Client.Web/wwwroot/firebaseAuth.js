//window.firebaseAuth = {
//    register: async function (email, password) {
//        if (!firebase.apps.length) {
//            firebase.initializeApp(window.firebaseConfig);
//        }

//        try {
//            const userCredential = await firebase.auth().createUserWithEmailAndPassword(email, password);
//            return await userCredential.user.getIdToken();
//        } catch (err) {
//            const code = err.code || "unknown_error";
//            throw code;
//            //throw new Error(code);
//        }
//    },

//    login: async function (email, password) {
//        if (!firebase.apps.length) {
//            firebase.initializeApp(window.firebaseConfig);
//        }

//        try {
//            const userCredential = await firebase.auth().signInWithEmailAndPassword(email, password);
//            return await userCredential.user.getIdToken();
//        } catch (err) {
//            const code = err.code || "unknown_error";
//            throw code;
//            //throw new Error(code);
//        }
//    }
//};


window.firebaseAuth = {
    register: async function (email, password) {
        if (!firebase.apps.length) {
            firebase.initializeApp(window.firebaseConfig);
        }

        try {
            const userCredential = await firebase.auth().createUserWithEmailAndPassword(email, password);
            const token = await userCredential.user.getIdToken();
            return { success: true, token: token, error: null }; // Zwracamy obiekt
        } catch (err) {
            return { success: false, token: null, error: err.code || "unknown_error" }; // Nie rzucamy b³êdu!
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
    }
};