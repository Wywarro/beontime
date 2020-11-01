<template>
  <nav class="topbar-nav">
    <font-awesome-icon type="fas" icon="bars" />
    <router-link to="/">Login</router-link>
    <router-link to="/">Register</router-link>
  </nav>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "Topbar",

});
</script>

<style scoped lang="postcss">
.topbar-nav {
  @apply flex fixed w-full items-center justify-between;
  @apply px-6 h-16 z-10 border-b;
  @apply border-gray-200 bg-white text-gray-700;
}
</style>
