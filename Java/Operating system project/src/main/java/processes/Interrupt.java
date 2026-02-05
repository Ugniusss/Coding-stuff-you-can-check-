package processes;

import core.Logger;
import core.Process;

import resources.PageTableResource;
import resources.ResultResource;

public class Interrupt extends Process {

    private int step = 1;

    public Interrupt() {
        this.pID = "Interrupt";
        this.priority = 99;
    }

    @Override
    public void execute() {
        //Logger.log("Executing process: " + this);

        switch (step) {
            case 1:
                //Logger.log("[Interrupt] Waiting for interrupt buffer resource");
                kernel.requestResource(this, "InterruptResource");
                step = 2;
                break;

            case 2:
                //Logger.log("[Interrupt] Determining interrupt type");
                //Logger.log("[Interrupt] Notifying related JobGovernor");
                kernel.createResource(this, new ResultResource());

                //Logger.log("[Interrupt] Ready for next interrupt");
                step = 1;
                break;
        }
    }
}
