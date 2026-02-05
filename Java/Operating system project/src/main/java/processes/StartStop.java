package processes;

import core.Logger;
import core.Process;
import core.Kernel;
import resources.*;

public class StartStop extends Process {

    private int step = 1;

    public StartStop() {
        this.pID = "StartStop";
        this.priority = 100;
    }

    @Override
    public void execute() {
        //Logger.log("Executing process: " + this);

        switch (step) {
            case 1:
                //Logger.log("[StartStop] Sukuriam resources");
                kernel.createResource(this, new ExecuteFlagResource());
                kernel.createResource(this, new ResultResource());
                kernel.createResource(this, new MOSproc());
                kernel.createResource(this, new PageTableResource());
                kernel.createResource(this, new SupervMemoryblockResource());
                kernel.createResource(this, new ExecuteFlagResource());
                kernel.createResource(this, new UserMemoryResource());
                kernel.createResource(this, new TaskInSupervMemoryResource());
                kernel.createResource(this, new WaitForInputResource());
                kernel.createResource(this, new WaitForOutputResource());
                step = 2;
                break;

            case 2:
                //Logger.log("[StartStop] Sukuriam processes");
                kernel.createProcess(this, new MainProc());
                kernel.createProcess(this, new MemoryGovernor());
                kernel.createProcess(this, new Interrupt());
                kernel.createProcess(this, new PrintLine());
                kernel.createProcess(this, new ReadFromKey());
                step = 3;
                break;

            case 3:
                //Logger.log("[StartStop] Blocking until MOS shutdown");
                kernel.requestResource(this, "MOSproc");
                break;

            default:
                //Logger.log("[StartStop] Invalid step: " + step);
        }
    }
}
