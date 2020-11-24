import { shallowMount } from "@vue/test-utils";

import WorkCalendar from "@/views/WorkCalendar.vue";

describe("WorkCalendar.vue", () => {
    it("renders props.msg when passed", () => {
        const msg = "new message";
        const wrapper = shallowMount(WorkCalendar, {
            props: { msg },
        });
        expect(wrapper.text()).toMatch(msg);
    });
});
