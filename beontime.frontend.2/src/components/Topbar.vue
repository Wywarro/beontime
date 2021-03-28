<template>
  <nav class="topbar-nav">
    <button
      class="hamburger hamburger--elastic hamburger-icon"
      :class="{ 'is-active': drawerOpened }"
      type="button"
      @click="toggleDrawer"
    >
      <span class="hamburger-box">
        <span class="hamburger-inner"></span>
      </span>
    </button>
    <div>
      <router-link class="mr-6" :to="{ name: 'login' }">Login</router-link>
      <router-link :to="{ name: 'signup' }">Sign up</router-link>
    </div>
  </nav>
</template>

<script lang="ts">
import { Options, Vue } from "vue-class-component";

@Options({
  props: {
    drawerOpened: {
      type: Boolean,
      required: true,
      default: false
    }
  },
  emits: {
    "update:drawerOpened": (drawerOpened: boolean) => {
      if (typeof drawerOpened === "boolean") {
        return true;
      } else {
        console.warn("Invalid drawerOpened payload!");
        return false;
      }
    }
  }
})
export default class Topbar extends Vue {
  drawerOpened: Boolean;

  toggleDrawer() {
    this.$emit("update:drawerOpened", !this.drawerOpened);
  }
}
</script>

<style lang="less" scoped>
.topbar-nav {
  display: flex;
  position: fixed;

  @apply w-full;
  @apply items-center;
  @apply justify-between;
  @apply px-6;
  @apply h-16;
  @apply z-10;
  @apply border-b;
  @apply border-gray-200;
  @apply bg-white;
  @apply text-gray-700;
}

.hamburger-icon {
  transform: scale(0.8);
  vertical-align: middle;
  padding-left: 0px;
  height: 100%;
  display: flex;
  align-items: center;

  &:active {
    outline: 0px;
  }

  &:focus {
    outline: 0px;
  }
}
</style>
