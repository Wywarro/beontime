<template>
  <div class="signup__container">
    <h3>Signup</h3>

    <form @submit.prevent="signupUser">
      <div class="email">
        <input v-model="emailAddress" type="email" placeholder="email" />
      </div>
      <div class="password">
        <input v-model="password" type="password" placeholder="password" />
      </div>
      <div class="password">
        <input
          v-model="repeatPassword"
          type="password"
          placeholder="repeat your password"
        />
      </div>
      <button type="submit">Signup</button>
    </form>

    <div v-if="error" class="error">{{ error.message }}</div>
  </div>
</template>

<script lang="ts">
import { Vue, setup } from "vue-class-component";

import { useVuelidate } from "@vuelidate/core";
import { email, required, minLength } from "@vuelidate/validators";

import firebase from "firebase/app";

export default class Signup extends Vue {
  emailAddress = "";
  password = "";
  repeatPassword = "";

  passwordMinLength = 6;

  requestError = "";

  $v = setup(() => {
    const rules = {
      password: { required, minLength: minLength(this.passwordMinLength) },
      emailAddress: { required, email },
    };

    return useVuelidate(rules, {
      password: this.password,
      emailAddress: this.emailAddress,
    });
  });

  get error(): string {
    if (!this.$v.$dirty) return "";

    if (this.$v.password.required.$invalid) {
      return "Hasło jest wymagane!";
    } else if (this.$v.password.minLength.$invalid) {
      return `Hasło musi mieć conajmniej ${this.passwordMinLength} znaków`;
    } else if (this.$v.emailAddress.email.$invalid) {
      return "Niewłaściwy adres email!";
    } else if (this.$v.emailAddress.required.$invalid) {
      return "Adress email jest wymagany!";
    } else if (this.password != this.repeatPassword) {
      return "Hasła muszą być identyczne!";
    }

    return this.requestError;
  }

  async signupUser(): Promise<void> {
    this.requestError = "";
    this.$v.$touch();

    if (this.$v.$invalid) return;

    try {
      await firebase
        .auth()
        .createUserWithEmailAndPassword(this.emailAddress, this.password);

      this.$router.push({ name: "Reports" });
    } catch (er) {
      if (er.code == "auth/email-already-in-use") {
        this.requestError = "Użytkownik o podanym email'u już istnieje!";
      } else {
        this.requestError = er.message;
        console.error({ error: er });
      }
    }
  }
}
</script>

<style lang="less" scoped>
.signup {
  &__container {
    display: flex;
  }
}
</style>
