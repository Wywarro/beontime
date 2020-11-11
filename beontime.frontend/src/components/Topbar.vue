<template>
  <nav class="topbar-nav">
    <button
      class="hamburger hamburger--elastic hamburger-icon"
      :class="{'is-active': drawerOpened }"
      @click="toggleDrawer"
      type="button"
    >
      <span class="hamburger-box">
        <span class="hamburger-inner"></span>
      </span>
    </button>
    <div>
      <router-link class="mr-6" to="/">Login</router-link>
      <router-link to="/">Register</router-link>
    </div>
  </nav>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "Topbar",
  props: {
    drawerOpened: {
      type: Boolean,
      required: true,
      default: false,
    },
  },
  setup (props, context) {
    const toggleDrawer = () => {
      context.emit("update:drawerOpened", !props.drawerOpened);
    };

    return { toggleDrawer, };
  },
  emits: {
    "update:drawerOpened": (drawerOpened: boolean) => {
      if (typeof drawerOpened === "boolean") {
        return true;
      } else {
        console.warn("Invalid drawerOpened payload!");
        return false;
      }
    },
  },
});
</script>

<style scoped lang="postcss">
.topbar-nav {
  @apply flex w-full items-center justify-between;
  @apply px-6 h-16 z-10 border-b;
  @apply border-gray-200 bg-white text-gray-700;
}

</style>

<style lang="less" scoped>
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
