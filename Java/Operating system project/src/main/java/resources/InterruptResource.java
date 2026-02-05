package resources;

import core.Resource;

public class InterruptResource extends Resource {
    private String type;
    private int value;

    public InterruptResource() {
        this.rIDI = "InterruptResource";

    }

    public InterruptResource(String type, int value) {
        this();
        this.type = type;
        this.value = value;
    }

    public String getType() {
        return this.type;
    }

    public int getValue() {
        return this.value;
    }
}

