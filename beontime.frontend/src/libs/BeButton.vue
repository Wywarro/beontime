<template>
    <component
        :is="tag"
        :type="type"
        :disabled="disableButton"
        class="bebutton"
        :class="[btnClass, colorVariants]"
        :variant="variant"
        :variant-type="variantType"
        :size="size"
        :href="to"
        v-bind="$attrs"
    >
        <slot/>
    </component>
</template>

<script lang='ts'>
import { defineComponent, PropType } from "vue";

interface ButtonStyle {
    normal: (isDisabled: boolean) => string,
    outline: (isDisabled: boolean) => string,
}

interface ButtonStyles {
    primary: () => ButtonStyle,
    secondary: () => ButtonStyle,
    danger: () => ButtonStyle,
    success: () => ButtonStyle,
    warning: () => ButtonStyle,
    white: () => ButtonStyle
}

export default defineComponent({
    name: "BeButton",
    props: {
        href: {
            type: String,
            default: null
        },
        to: {
            type: String,
            default: null
        },
        disabled: {
            type: Boolean,
            default: false
        },
        variant: {
            type: String as PropType<string>,
            default: "primary"
        },
        variantType: {
            type: String,
            default: "normal"
        },
        size: {
            type: String,
            default: "normal"
        },
        rounded: {
            type: String,
            default: "medium"
        },
        type: {
            type: String,
            default: ""
        },
    },
    data() {
        return {
            loading: false,
            disableButton: this.disabled
        };
    },
    computed: {
        tag() {
            if (this.href) {
                return "a";
            }
            return "button";
        },
        btnClass(): any {
            return {
                "base-spinner": this.loading === true,
                "cursor-not-allowed": this.disableButton === true,
                "base-button inline-flex align-middle align-items-center justify-center font-medium focus:outline-none border-2": true,

                "rounded-lg": this.rounded === "medium",
                "rounded-full": this.rounded === "large",

                "px-6 py-3": this.size === "normal",
                "px-4 py-2": this.size === "small"
            };
        },
        colorVariants(): string {
            const variants: ButtonStyles = {
                primary: (): ButtonStyle => ({
                    normal: (isDisabled) => {
                        if (isDisabled) {
                            return "border-blue-300 bg-blue-300 text-white";
                        }
                        return "border-blue-600 bg-blue-600 hover:bg-blue-700 hover:border-blue-700 text-white";
                    },
                    outline: (isDisabled) => {
                        if (isDisabled) {
                            return "";
                        }
                        return "border-gray-200 text-blue-500 hover:text-blue-700";
                    }
                }),
                secondary: (): ButtonStyle => ({
                    normal: (isDisabled) => {
                        if (isDisabled) {
                            return "";
                        }
                        return "";
                    },
                    outline: (isDisabled) => {
                        if (isDisabled) {
                            return "";
                        }
                        return "border-gray-300 text-gray-500 hover:text-blue-500";
                    }
                }),
                danger: (): ButtonStyle => ({
                    normal: (isDisabled) => {
                        if (isDisabled) {
                            return "border-red-300 bg-red-300 text-white;";
                        }
                        return "border-red-600 bg-red-600 hover:bg-red-700 hover:border-red-700 text-white";
                    },
                    outline: (isDisabled) => {
                        if (isDisabled) {
                            return "";
                        }
                        return "border-gray-200 text-red-500 hover:text-red-600";
                    }
                }),
                warning: (): ButtonStyle => ({
                    normal: (isDisabled) => {
                        if (isDisabled) {
                            return "";
                        }
                        return "border-orange-600 bg-orange-600 hover:bg-orange-700 hover:border-orange-700 text-white";
                    },
                    outline: (isDisabled) => {
                        if (isDisabled) {
                            return "";
                        }
                        return "";
                    }
                }),
                success: (): ButtonStyle => ({
                    normal: (isDisabled) => {
                        if (isDisabled) {
                            return "border-green-300 bg-green-300 text-white";
                        }
                        return "border-green-600 bg-green-600 hover:bg-green-700 hover:border-green-700 text-white";
                    },
                    outline: (isDisabled) => {
                        if (isDisabled) {
                            return "";
                        }
                        return "border-2 border-gray-200 text-green-500 hover:text-green-700";
                    }
                }),
                white: (): ButtonStyle => ({
                    normal: (isDisabled) => {
                        if (isDisabled) {
                            return "border-white bg-white text-blue-600 hover:text-blue-800";
                        }
                        return "";
                    },
                    outline: (isDisabled) => {
                        if (isDisabled) {
                            return "";
                        }
                        return "";
                    }
                }),
            };

            const buttonStyle: ButtonStyle = variants[this.variant as keyof ButtonStyles]();

            return buttonStyle[this.variantType as keyof ButtonStyle](this.disableButton);
        }
    }
});
</script>

<style lang='less' scoped>
.bebutton {
    display: flex;
    align-items: center;

    @apply p-4;
    @apply shadow-xs;
    @apply cursor-pointer;
    @apply transition;
    @apply duration-200;

    &:focus {
        outline: 0;
    }

    &:hover {
        @apply bg-indigo-700;
    }
}
</style>
