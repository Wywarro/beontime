module.exports = {
    root: true,
    env: {
        node: true,
    },
    extends: [
        "plugin:vue/vue3-recommended",
        "@vue/standard",
        "@vue/typescript/recommended",
        "prettier/vue",
    ],
    parserOptions: {
        ecmaVersion: 2020,
    },
    rules: {
        "no-console": process.env.NODE_ENV === "production" ? "warn" : "off",
        "no-debugger": process.env.NODE_ENV === "production" ? "warn" : "off",
        quotes: [1, "double", { avoidEscape: true }],
        "comma-dangle": [
            "off",
            {
                arrays: "only-multiline",
                objects: "only-multiline",
                imports: "never",
                exports: "never",
                functions: "ignore",
            },
        ],
        semi: [1, "always"],
        indent: ["error", 4],
        "vue/singleline-html-element-content-newline": "off",
        "vue/multiline-html-element-content-newline": "off",
        "vue/require-default-prop": "off",
        "space-before-function-paren": ["error", "never"],
        "vue/html-indent": [
            "error",
            4,
            {
                attribute: 1,
                baseIndent: 1,
                closeBracket: 0,
                alignAttributesVertically: true,
                ignores: [],
            },
        ],
    },
    overrides: [
        {
            files: [
                "**/__tests__/*.{j,t}s?(x)",
                "**/tests/unit/**/*.spec.{j,t}s?(x)",
            ],
            env: {
                jest: true,
            },
        },
    ],
};
