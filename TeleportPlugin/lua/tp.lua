local function teleportExec(pos, dir)
  if supportAPI_physics then
    physics.setGentleStop(0, false)
  end

  physics.setCarPosition(0, pos, dir)
end

local tpEvent = ac.OnlineEvent(
  {
    ac.StructItem.key('AS_TeleportToPlayer'),
    position = ac.StructItem.vec3(),
    direction = ac.StructItem.vec3()
  },
  function(sender, message)
    if sender ~= nil then
      return
    end

    teleportExec(message.position, message.direction)
  end
)
