import { shallowMount, VueWrapper } from "@vue/test-utils";

import BeButton from "@/libs/BeButton.vue";

describe("BeButton.vue", () => {
    describe("general behaviour", () => {
        let wrapper: VueWrapper<any>;

        beforeEach(() => {
            wrapper = shallowMount(BeButton, {});
        });

        afterEach(() => {
            wrapper.unmount();
        });

        it("renders 23 divs with .cal__row", () => {
            const rows = wrapper.findAll(".cal__row");
            expect(rows.length).toBe(23);
        });
    });
});
