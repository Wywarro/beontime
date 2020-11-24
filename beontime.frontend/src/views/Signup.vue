<template>
    <div class="signup__container">
        <h3>Signup</h3>

        <form @submit.prevent="signupUser">
            <div class="email">
                <input
                    v-model="email"
                    type="email"
                    placeholder="email"
                />
            </div>
            <div class="password">
                <input
                    v-model="password"
                    type="password"
                    placeholder="password"
                />
            </div>
            <button type="submit">Signup</button>
        </form>

        <div
            v-if="error"
            class="error"
        >{{ error.message }}</div>
    </div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import { useRouter } from "vue-router";

import firebase from "firebase";

export default defineComponent({
    name: "Signup",
    setup() {
        const email = ref("");
        const password = ref("");
        const error = ref("");

        const router = useRouter();
        const signupUser = async(): Promise<void> => {
            try {
                const user = await firebase
                    .auth()
                    .createUserWithEmailAndPassword(email.value, password.value);

                console.log(user);
                router.replace({ name: "home" });
            } catch (error) {
                error.value = error;
            }
        };

        return { email, password, error, signupUser };
    }
});
</script>

<style lang="less" scoped>
.signup {
    &__container {
        display: flex;
    }
}
</style>
